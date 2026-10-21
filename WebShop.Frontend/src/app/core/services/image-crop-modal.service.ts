import { Injectable, signal } from '@angular/core';

export interface ImageCropModalData {
  file: File;
  onCropped: (croppedFile: File) => void;
}

@Injectable({ providedIn: 'root' })
export class ImageCropModalService {
  private _data = signal<ImageCropModalData | null>(null);
  data = this._data.asReadonly();

  open(data: ImageCropModalData): void {
    this._data.set(data);
  }

  close(): void {
    this._data.set(null);
  }
}
