import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  get404Error() {
    this.http.get(this.baseUrl + 'Buggy/not-found').subscribe(
      res => {
        console.log(res)
      }, error => console.log(error)
    );
  }

  get400Error() {
    this.http.get(this.baseUrl + 'Buggy/bad-request').subscribe(
      res => {
        console.log(res)
      }, error => console.log(error)
    );
  }

  get401Error() {
    this.http.get(this.baseUrl + 'Buggy/auth').subscribe(
      res => {
        console.log(res)
      }, error => console.log(error)
    );
  }

  get400ValidationError() {
    this.http.post(this.baseUrl + 'account/register', {}).subscribe(
      res => {
        console.log(res)
      }, error => console.log(error)
    );
  }
  get500Error() {
    this.http.get(this.baseUrl + 'Buggy/server-error').subscribe(
      res => {
        console.log(res)
      }, error => console.log(error)
    );
  }
}
