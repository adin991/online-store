export interface CurrentUserDto {
  userId: number;
  email: string;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  tokenVersion: number;
  name?: string;
  surname?: string;
  gender?: number;
}
