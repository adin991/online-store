import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ChatTurn {
  role: 'user' | 'model';
  text: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatApiService {
  private readonly baseUrl = `${environment.apiUrl}/api/chat`;

  /**
   * POST /api/chat/stream
   * Streams the assistant's reply as plain-text chunks over the raw HTTP response body.
   * Uses fetch() + ReadableStream directly since Angular's HttpClient does not expose
   * a progressive read of the response body while it's still arriving.
   */
  sendMessage(message: string, history: ChatTurn[]): Observable<string> {
    return new Observable<string>((subscriber) => {
      const controller = new AbortController();

      fetch(`${this.baseUrl}/stream`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ message, history }),
        signal: controller.signal
      })
        .then(async (response) => {
          if (!response.ok || !response.body) {
            subscriber.error(new Error(`Chat request failed (${response.status}).`));
            return;
          }

          const reader = response.body.getReader();
          const decoder = new TextDecoder();

          for (;;) {
            const { done, value } = await reader.read();
            if (done) break;
            subscriber.next(decoder.decode(value, { stream: true }));
          }

          subscriber.complete();
        })
        .catch((err) => subscriber.error(err));

      return () => controller.abort();
    });
  }
}
