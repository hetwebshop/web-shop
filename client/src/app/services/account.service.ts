import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Params } from '@angular/router';
import {
  BehaviorSubject,
  catchError,
  debounceTime,
  first,
  map,
  of,
  switchMap,
  tap,
  throwError,
} from 'rxjs';
import { environment } from 'src/environments/environment';
import { Address, LocationInfo } from '../modal/address';
import { User, UserInfo } from '../modal/user';
import { HttpService } from './http.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private userSource = new BehaviorSubject<User>(null);
  user$ = this.userSource.asObservable();
  baseUrl = environment.apiUrl + 'account/';
  loggedIn: boolean;

  constructor(private http: HttpService) {
    this.user$.subscribe((u) => {
      this.loggedIn = u !== null;
    });
  }

  login(model: any) {
    return this.http.post<User>(this.baseUrl + 'login', model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
        }
      })
    );
  }

  register(model: any) {
    const payload = { ...model, userName: model.Email };
  
    return this.http.post<User>(`${this.baseUrl}register`, payload).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
        }
        return user;
      }),
      catchError((error) => {
        console.error('Greška prilikom registracije', error);
        return throwError(() => new Error(error));
      })
    );
  }
  
  registerCompany(model) {
    model.userName = model.email;
    return this.http.post<User>(this.baseUrl + 'register-company', model).pipe(
      map((user: User) => {
        if (user) {
          this.setUser(user);
        }
      })
    );
  }

  getProfile() {
    return this.http.get(this.baseUrl + 'profile');
  }

  getAddress() {
    return this.http
      .get<Address>(this.baseUrl + 'address')
      .pipe(tap((response) => this.updateLocationCache(response)));
  }

  updateAddress(body) {
    return this.http
      .post<Address>(this.baseUrl + 'address', body)
      .pipe(tap((response) => this.updateLocationCache(response)));
  }

  updateLocationCache(address: Address) {
    this.http.setCache(
      address.locations.areas,
      this.baseUrl + 'location-list',
      {
        parentId: address.cityId,
        childType: 'Area',
      }
    );
    this.http.setCache(
      address.locations.cities,
      this.baseUrl + 'location-list',
      {
        parentId: address.stateId,
        childType: 'City',
      }
    );
    this.http.setCache(
      address.locations.states,
      this.baseUrl + 'location-list',
      {
        parentId: address.countryId,
        childType: 'State',
      }
    );
  }

  getLocations(params: Params) {
    return this.http.get<LocationInfo[]>(this.baseUrl + 'location-list', {
      params,
      cache: true,
      background: true,
    });
  }

  updateProfile(formData: FormData) {
    return this.http.post<any>(this.baseUrl + 'profile', formData).pipe(
      map((response) => {
        // Perform any additional actions on response if needed
        return response;
      })
    );
  }

  updateCompanyProfile(formData: FormData) {
    console.log("COMPANY IS", formData);
    return this.http.post<any>(this.baseUrl + 'company-profile', formData).pipe(
      map((response) => {
        return response;
      })
    );
  }

  updateToken() {
    this.http.get<User>(this.baseUrl + 'token-update').subscribe((user) => {
      if (user) {
        this.setUser(user);
      }
    });
  }

  changePhoto(fileToUpload: File) {
    const formData: FormData = new FormData();
    formData.append('file', fileToUpload);
    return this.http.post<any>(this.baseUrl + 'change-photo', formData).pipe(
      map((response) => {
        this.updateToken();
        return response;
      })
    );
  }

  removePhoto() {
    const formData: FormData = new FormData();
    formData.append('remove', 'true');
    return this.http.post<any>(this.baseUrl + 'change-photo', formData).pipe(
      map((response) => {
        this.updateToken();
        return response;
      })
    );
  }

  logout() {
    localStorage.removeItem('user');
    this.userSource.next(null);
  }

  userExist(userName: string) {
    return this.http.get(this.baseUrl + userName, {
      background: true,
    });
  }

  userExistAsync(exist = true, currentUserName = ''): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (control?.value && control.dirty) {
        if (currentUserName === control.value) return of(null);
        return control.valueChanges
          .pipe(
            debounceTime(2000),
            switchMap((value) => this.userExist(value)),
            map((response) => {
              if (response.toString() === exist.toString()) return null;
              return { alreadyExist: response };
            })
          )
          .pipe(first());
      }
      return of(null);
    };
  }

  getUserInfo(userName: string) {
    return this.http.get<UserInfo>(this.baseUrl + 'user/' + userName, {
      background: true,
    });
  }

  setUser(user: User) {
    user.roles = [];
    const decodedToken = this.getDecodedToken(user.token);
    const roles = decodedToken.role;
    user.id = parseInt(decodedToken.nameid);
    Array.isArray(roles) ? (user.roles = roles) : (user.roles = [roles]);
    localStorage.setItem('user', JSON.stringify(user));
    //this.userSource.next(null);
    this.userSource.next(user);
  }

  getDecodedToken(token: string) {
    return JSON.parse(atob(token.split('.')[1]));
  }
}
