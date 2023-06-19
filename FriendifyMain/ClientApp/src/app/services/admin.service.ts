import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AdminData } from '../models/admin-data';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  // Inject the HttpService in the constructor
  constructor(private httpService: HttpService) { }

  // Define a method to get the admin data from the API
  getAdminData(selectedTime?: Date): Observable<AdminData> {
    // If no date is passed, use the current year as the default value
    if (!selectedTime) {
      selectedTime = new Date();
      selectedTime.setMonth(0); // Set the month to January
      selectedTime.setDate(1); // Set the date to the first day of the month
    }
    // Use the HttpService to make a GET request and return the result as an observable of AdminData
    return this.httpService.get(`/admin?selectedTime=${selectedTime.toISOString()}`);
  }
}
