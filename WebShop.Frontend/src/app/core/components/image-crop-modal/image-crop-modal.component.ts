import { Component, inject } from '@angular/core';
import { ImageCroppedEvent } from 'ngx-image-cropper';
import { ImageCropModalService } from '../../services/image-crop-modal.service';

@Component({
  selector: 'app-image-crop-modal',
  standalone: false,
  templateUrl: './image-crop-modal.component.html',
  styleUrl: './image-crop-modal.component.scss'
})
export class ImageCropModalComponent {
  modal = inject(ImageCropModalService);

  private croppedBlob: Blob | null = null;

  onImageCropped(event: ImageCroppedEvent): void {
    this.croppedBlob = event.blob ?? null;
  }

  confirm(): void {
    const data = this.modal.data();
    if (!data || !this.croppedBlob) return;

    const croppedFile = new File(
      [this.croppedBlob],
      data.file.name.replace(/\.[^.]+$/, '') + '.jpg',
      { type: this.croppedBlob.type }
    );

    data.onCropped(croppedFile);
    this.reset();
  }

  cancel(): void {
    this.reset();
  }

  private reset(): void {
    this.croppedBlob = null;
    this.modal.close();
  }
}
