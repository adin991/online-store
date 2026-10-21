import { Component, inject } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ChatApiService, ChatTurn } from '../../../api-services/chat/chat-api.service';
import { ToasterService } from '../../services/toaster.service';
import { LocaleService } from '../../services/locale.service';
import { AppValidators } from '../../validators/app-validators';

@Component({
  selector: 'app-chat-widget',
  standalone: false,
  templateUrl: './chat-widget.component.html',
  styleUrl: './chat-widget.component.scss'
})
export class ChatWidgetComponent {
  isOpen = false;
  isSending = false;
  messages: ChatTurn[] = [];

  draft = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, AppValidators.notBlank, Validators.maxLength(2000)]
  });

  private chatApi = inject(ChatApiService);
  private toaster = inject(ToasterService);
  private locale = inject(LocaleService);

  toggle(): void {
    this.isOpen = !this.isOpen;
  }

  send(): void {
    if (this.draft.invalid || this.isSending) return;
    const text = this.draft.value.trim();

    const history = [...this.messages];
    this.messages.push({ role: 'user', text });
    this.draft.reset();
    this.isSending = true;
    this.draft.disable();

    const reply: ChatTurn = { role: 'model', text: '' };
    this.messages.push(reply);

    this.chatApi.sendMessage(text, history).subscribe({
      next: (chunk) => {
        reply.text += chunk;
      },
      error: (err) => {
        this.finishSending();
        this.toaster.error(err?.message ?? this.locale.t('CHAT.SEND_FAILED'));
      },
      complete: () => this.finishSending()
    });
  }

  private finishSending(): void {
    this.isSending = false;
    this.draft.enable();
  }
}
