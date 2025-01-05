import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {


  model: any = {};
  defaultPhoto = '';

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
