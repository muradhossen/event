import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { NodeWithI18n } from '@angular/compiler';
import { Router } from '@angular/router';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  @Input() usersFromHomeComponent;
  @Output() calcelRegister = new EventEmitter();

  model: any = {};
  registerForm: FormGroup;
  maxDate : Date;
  validationErrors : string[] = [];

  constructor(private accountService: AccountService,
    private toastr: ToastrService, private fb: FormBuilder, private router : Router) { }

  ngOnInit(): void {

    this.initializeForm();

    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male', Validators.required],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', [Validators.required, this.matchValues('password')]]
    })
  }
  matchValues(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {

      return control?.value === control?.parent?.controls[matchTo].value ? null : { isMatching: true }
    }
  }
  register() { 
 
    this.accountService.register(this.registerForm.value).subscribe(response => {

      this.router.navigateByUrl('/members');
      
    }, error => {
     
     this.validationErrors = error;
    })
  }
  cancel() {
    this.calcelRegister.emit(false);
  }
}
