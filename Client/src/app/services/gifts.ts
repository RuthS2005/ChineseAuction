import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
;

// =====================
// 1. Interfaces (חייבים להתאים ל-DTO בשרת)
// =====================

export interface Gift {
  id: number;
  name: string;
  description: string;
  category: string;
  imageUrl: string;
  cost: number;
  donorId: number;
}

export interface CreateGiftDto {
  // id is omitted because server usually creates it
  name: string;
  description: string;
  category: string;
  imageUrl?: string;
  cost: number;
  donorId: number;
}

export interface Donor {
  id: number;
  name: string;
  email: string;
  phone: string;
}

export interface CreateDonorDto {
  name: string;
  email: string;
  phone?: string;
}

// זה ה-DTO שאנחנו שולחים לקנייה
export interface PurchaseDto {
  userId: number;
  giftId: number;
  quantity: number;
}

export interface CartItem {
  // adapt fields to server response
  id: number;
  giftId: number;
  quantity: number;
  name?: string;
  cost?: number;
}

export interface RaffleResult {
  giftId: number;
  winnerUserId?: number;
  message?: string;
  winnerName: string;
  email: string;
}

export interface User {
  id: number;
  name: string;
  email: string;
  role: string;
}

// =====================
// 2. הסרוויס (הצינור לשרת)
// =====================

@Injectable({
  providedIn: 'root'
})
export class GiftsService {

  // 👇 use environment; fallback to previous hardcoded value if not set
  private apiUrl =  'http://localhost:5035/api';

  constructor(private http: HttpClient) { }

  // small helper to build URLs and avoid accidental double-slashes
  private url(path: string) {
    const base = this.apiUrl.replace(/\/+$/, '');
    const p = path.replace(/^\/+/, '');
    return `${base}/${p}`;
  }

  // centralized error handler (returns an observable error)
  private handleError(operation = 'operation') {
    return (error: any) => {
      // TODO: replace console.error with a proper logger if available
      console.error(`${operation} failed:`, error);
      return throwError(() => new Error(`${operation} failed: ${error?.message || error}`));
    };
  }

  // =====================
  // A. ניהול מתנות
  // =====================

  getGifts(): Observable<Gift[]> {
    return this.http.get<Gift[]>(this.url('Gift'))
      .pipe(catchError(this.handleError('getGifts')));
  }

  // שים לב: אנחנו לא עושים push למערך, אלא שולחים לשרת
  // Use CreateGiftDto for request and expect created Gift (with id) back from server
  addGift(gift: CreateGiftDto): Observable<Gift> {
    if (!gift || !gift.name || gift.cost == null) {
      return throwError(() => new Error('Invalid gift data'));
    }
    return this.http.post<Gift>(this.url('Gift'), gift)
      .pipe(catchError(this.handleError('addGift')));
  }

  deleteGift(id: number): Observable<void> {
    if (id == null || isNaN(id)) {
      return throwError(() => new Error('id is required'));
    }
    return this.http.delete<void>(this.url(`Gift/${id}`))
      .pipe(catchError(this.handleError('deleteGift')));
  }

  updateGift(gift: Gift): Observable<Gift> {
    if (!gift || gift.id == null || isNaN(gift.id)) {
      return throwError(() => new Error('Invalid gift data'));
    }     
    return this.http.put<Gift>(this.url(`Gift/${gift.id}`), gift)
      .pipe(catchError(this.handleError('updateGift')));
  }
    raffle(giftId: number): Observable<RaffleResult> {
    if (giftId == null || isNaN(giftId)) {
      return throwError(() => new Error('giftId is required'));
    }
    return this.http.post<RaffleResult>(this.url(`Gift/raffle/${giftId}`), {})
      .pipe(catchError(this.handleError('raffle')));
  }

  // =====================
  // B. ניהול תורמים
  // =====================

  getDonors(): Observable<Donor[]> {
    return this.http.get<Donor[]>(this.url('Donors'))
      .pipe(catchError(this.handleError('getDonors')));
  }

  addDonor(donor: CreateDonorDto): Observable<Donor> {
    if (!donor || !donor.name || !donor.email) {
      return throwError(() => new Error('Invalid donor data'));
    }
    return this.http.post<Donor>(this.url('Donors'), donor)
      .pipe(catchError(this.handleError('addDonor')));
  }

  deleteDonor(id: number): Observable<void> {
    if (id == null || isNaN(id)) {
      return throwError(() => new Error('id is required'));
    }
    return this.http.delete<void>(this.url(`Donors/${id}`))
      .pipe(catchError(this.handleError('deleteDonor')));
  }
updateDonor(donor: any): Observable<any> {
  // שימוש ב-apiUrl שלנו, ובלי ה-handleError המורכב כרגע
  return this.http.put(`${this.apiUrl}/Donors/${donor.id}`, donor);
}
  // =====================
  // C. סל קניות (הלוגיקה עברה לשרת!)
  // =====================

  // שליפת הסל של משתמש ספציפי
  getCart(userId: number): Observable<CartItem[]> {
    if (userId == null || isNaN(userId)) {
      return throwError(() => new Error('userId is required'));
    }
    return this.http.get<CartItem[]>(this.url(`Purchases/Cart/${userId}`))
      .pipe(catchError(this.handleError('getCart')));
  }

  // הוספה לסל
  addToCart(purchase: PurchaseDto): Observable<any> {
    if (!purchase || purchase.userId == null || purchase.giftId == null || purchase.quantity == null) {
      return throwError(() => new Error('Invalid purchase data'));
    }
    return this.http.post(this.url('Purchases/AddToCart'), purchase)
      .pipe(catchError(this.handleError('addToCart')));
  }

  // תשלום (Checkout)
  checkout(userId: number): Observable<any> {
    if (userId == null || isNaN(userId)) {
      return throwError(() => new Error('userId is required'));
    }
    return this.http.post(this.url(`Purchases/Checkout/${userId}`), {})
      .pipe(catchError(this.handleError('checkout')));
  }


}