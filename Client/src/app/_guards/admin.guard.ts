import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AdminGuard implements CanActivate {

constructor(private accountService : AccountService,private toster : ToastrService ){}

  canActivate(): Observable<boolean> {
   return this.accountService.currentUser$.pipe(map(user => {
      if (user.roles.includes("Admin") || user.roles.includes("Moderator")) {
        return true;
      }
      this.toster.error("You cannot enter enter this area!"); 
    }));

    
  }
  
}
