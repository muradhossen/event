import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};
defaultPhoto = ''

  constructor(public accountServic: AccountService, private router: Router,
    private toastr: ToastrService, 
  private cd : ChangeDetectorRef) { }

  ngOnInit(): void {
  }

  login() {
    this.accountServic.login(this.model, this.cd).subscribe(response => {
      console.log(response);
      this.router.navigateByUrl('/');
    }, error => {
      console.log(error);
      this.toastr.error(error.error);
    })
  }

  logout() {
    this.accountServic.logout();
    this.router.navigateByUrl('/');
  }
}
