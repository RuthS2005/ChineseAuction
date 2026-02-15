import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
;

// =====================
//     Interfaces
// =====================

export interface Gift {
  id: number;
  name: string;
  description: string;
  category: string;
  imageUrl: string;
  cost: number;
  donorId: number;
  winnerName?: string; // זה חייב להיות כאן!
  // ואם אתה רוצה למיין לפי פופולריות, כדאי להוסיף גם:
  ticketCount?: number;
}
export interface CreateGiftDto {
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

export interface PurchaseDto {
  userId: number;
  giftId: number;
  quantity: number;
}

export interface CartItem {
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
//       הסרוויס
// =====================

@Injectable({
  providedIn: 'root'
})
export class GiftsService {

  private apiUrl =  'http://localhost:5035/api';

  constructor(private http: HttpClient) { }

  private url(path: string) {
    const base = this.apiUrl.replace(/\/+$/, '');
    const p = path.replace(/^\/+/, '');
    return `${base}/${p}`;
  }

  private handleError(operation = 'operation') {
    return (error: any) => {
      console.error(`${operation} failed:`, error);
      return throwError(() => new Error(`${operation} failed: ${error?.message || error}`));
    };
  }

  // =====================
  //    ניהול מתנות
  // =====================

getGifts(search?: string, sort?: string): Observable<Gift[]> {
  
  let url = `${this.apiUrl}/Gift`;

  if (search) {
    url += `search=${search}&`;
  }
  if (sort) {
    url += `sort=${sort}&`;
  }

  return this.http.get<Gift[]>(url);
}
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

getDonors(searchQuery: string = ''): Observable<Donor[]> {
  return this.http.get<Donor[]>(`${this.apiUrl}/Donors?search=${searchQuery}`);
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
// דוח זוכים
  getWinnersReport(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/Reports/winners`);
  }
  
  // דוח הכנסות
  getIncomeReport(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/Reports/income`);
  }

}