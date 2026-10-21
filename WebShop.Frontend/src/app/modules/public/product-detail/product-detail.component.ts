import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdsApiService, AdDetailDto } from '../../../api-services/ads/ads-api.service';
import { COMMENT_MAX_LENGTH, CommentsApiService, CommentDto, ListCommentsRequest } from '../../../api-services/comments/comments-api.service';
import { PublicUserProfileDto, UserApiService } from '../../../api-services/user/user-api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CartService } from '../../../core/services/cart/cart.service';
import { StripePaymentModalService } from '../../../core/services/stripe-payment-modal.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { LocaleService } from '../../../core/services/locale.service';
import { AppValidators } from '../../../core/validators/app-validators';
import { pageCount } from '../../../core/models/paging/page-result';

@Component({
  selector: 'app-product-detail',
  standalone: false,
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  ad: AdDetailDto | null = null;
  seller: PublicUserProfileDto | null = null;
  selectedImage = 0;
  loading = true;
  notFound = false;
  isDeleting = false;

  // Selected product options
  selectedColor: string | null = null;
  selectedSize: string | null = null;
  optionsError = false;

  // Comments (paged)
  comments: CommentDto[] = [];
  commentsTotal = 0;
  commentsRequest = new ListCommentsRequest();
  isSubmittingComment = false;
  editingCommentId: number | null = null;
  readonly commentMaxLength = COMMENT_MAX_LENGTH;

  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private adsApi = inject(AdsApiService);
  private commentsApi = inject(CommentsApiService);
  private userApi = inject(UserApiService);
  private cart = inject(CartService);
  private stripePaymentModal = inject(StripePaymentModalService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);
  auth = inject(AuthFacadeService);

  private commentValidators = [Validators.required, AppValidators.notBlank, Validators.maxLength(COMMENT_MAX_LENGTH)];

  /** New comment form */
  commentForm = this.fb.nonNullable.group({
    content: ['', this.commentValidators]
  });

  /** Inline edit form of an existing comment */
  editForm = this.fb.nonNullable.group({
    content: ['', this.commentValidators]
  });

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) { this.router.navigate(['/']); return; }

    this.commentsRequest.adId = id;
    this.commentsRequest.paging.pageSize = 5;

    this.adsApi.getById(id).subscribe({
      next: (ad) => {
        this.ad = ad;
        this.loading = false;
        this.selectedColor = ad.colors.length === 1 ? ad.colors[0] : null;
        this.selectedSize = ad.sizes.length === 1 ? ad.sizes[0] : null;
        this.loadComments();
        this.userApi.getPublicProfile(ad.userId).subscribe({ next: (p) => this.seller = p });
      },
      error: () => { this.notFound = true; this.loading = false; }
    });
  }

  // ---------- options ----------

  get isOwner(): boolean {
    const user = this.auth.currentUser();
    return !!user && !!this.ad && user.userId === this.ad.userId;
  }

  /** The ad can be put in the cart / bought: available and not the user's own ad. */
  get canBuy(): boolean {
    return !!this.ad && this.ad.isAvailable && !this.isOwner;
  }

  private optionsSelected(): boolean {
    if (!this.ad) return false;
    const ok = (this.ad.colors.length === 0 || !!this.selectedColor)
            && (this.ad.sizes.length === 0 || !!this.selectedSize);
    this.optionsError = !ok;
    return ok;
  }

  // ---------- comments ----------

  get commentsPages(): number {
    return pageCount(this.commentsTotal, this.commentsRequest.paging.pageSize);
  }

  loadComments(): void {
    this.commentsApi.list(this.commentsRequest).subscribe({
      next: (res) => {
        this.comments = res.items ?? [];
        this.commentsTotal = res.total ?? 0;
      },
      error: () => this.toaster.error(this.locale.t('COMMENTS.LOAD_FAILED'))
    });
  }

  goToCommentsPage(page: number): void {
    this.commentsRequest.paging.page = page;
    this.loadComments();
  }

  changeCommentsPageSize(size: number): void {
    this.commentsRequest.paging.pageSize = size;
    this.commentsRequest.paging.page = 1;
    this.loadComments();
  }

  submitComment(): void {
    this.commentForm.markAllAsTouched();
    if (this.commentForm.invalid || !this.ad || this.isSubmittingComment) return;

    this.isSubmittingComment = true;
    const content = this.commentForm.getRawValue().content.trim();

    this.commentsApi.create(this.ad.id, content).subscribe({
      next: () => {
        this.commentForm.reset();
        this.isSubmittingComment = false;
        // oldest first -> the new comment is on the last page
        this.commentsTotal++;
        this.goToCommentsPage(this.commentsPages);
      },
      error: (err) => {
        this.isSubmittingComment = false;
        this.toaster.error(err?.error?.message ?? this.locale.t('COMMENTS.SAVE_FAILED'));
      }
    });
  }

  startEdit(comment: CommentDto): void {
    this.editingCommentId = comment.id;
    this.editForm.reset({ content: comment.content });
  }

  cancelEdit(): void {
    this.editingCommentId = null;
    this.editForm.reset();
  }

  saveEdit(comment: CommentDto): void {
    this.editForm.markAllAsTouched();
    if (this.editForm.invalid) return;

    const content = this.editForm.getRawValue().content.trim();
    this.commentsApi.update(comment.id, content).subscribe({
      next: () => {
        comment.content = content;
        this.cancelEdit();
      },
      error: (err) => this.toaster.error(err?.error?.message ?? this.locale.t('COMMENTS.SAVE_FAILED'))
    });
  }

  deleteComment(comment: CommentDto): void {
    if (!confirm(this.locale.t('COMMENTS.DELETE_CONFIRM'))) return;
    this.commentsApi.delete(comment.id).subscribe({
      next: () => {
        if (this.comments.length === 1 && this.commentsRequest.paging.page > 1) {
          this.commentsRequest.paging.page--;
        }
        this.loadComments();
      },
      error: (err) => this.toaster.error(err?.error?.message ?? this.locale.t('COMMON.DELETE_FAILED'))
    });
  }

  canManageComment(comment: CommentDto): boolean {
    const user = this.auth.currentUser();
    if (!user) return false;
    return user.isAdmin || user.userId === comment.userId;
  }

  // ---------- ad actions ----------

  get canDelete(): boolean {
    const user = this.auth.currentUser();
    if (!this.ad || !user) return false;
    return user.isAdmin || user.userId === this.ad.userId;
  }

  deleteAd(): void {
    if (!this.ad || !confirm(this.locale.t('AD_DETAIL.DELETE_CONFIRM'))) return;
    this.isDeleting = true;
    this.adsApi.delete(this.ad.id).subscribe({
      next: () => this.router.navigate(['/']),
      error: () => { this.isDeleting = false; }
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  addToCart(): void {
    if (!this.ad || !this.canBuy) return;
    if (!this.auth.isAuthenticated()) {
      this.toaster.error(this.locale.t('AD_DETAIL.LOGIN_TO_ADD'));
      return;
    }
    if (!this.optionsSelected()) return;

    this.cart.add({
      id: this.ad.id,
      name: this.ad.title,
      price: this.ad.price,
      color: this.selectedColor,
      size: this.selectedSize
    });
    this.toaster.success(this.locale.t('AD_DETAIL.ADDED_TO_CART'));
  }

  buyNow(): void {
    if (!this.ad || !this.canBuy) return;
    if (!this.auth.isAuthenticated()) {
      this.toaster.error(this.locale.t('AD_DETAIL.LOGIN_TO_BUY'));
      return;
    }
    if (!this.optionsSelected()) return;

    const options = [this.selectedColor, this.selectedSize].filter(Boolean).join(', ');
    this.stripePaymentModal.open({
      adId: this.ad.id,
      title: options ? `${this.ad.title} (${options})` : this.ad.title,
      priceKm: this.ad.price,
      // the paid ad is closed by the API - reflect it right away
      onSuccess: () => { if (this.ad) this.ad = { ...this.ad, isAvailable: false, status: 'Closed' }; }
    });
  }
}
