import { Injectable } from '@angular/core';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal'
import { ConfirmDialogComponent } from '../modals/confirm-dialog/confirm-dialog.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef: BsModalRef;

  constructor(public modalService: BsModalService) { }

  confirm(title = "Confirmation",
    message = "Are you sure you want to do this?",
    btnOkText = "OK",
    btnCancelText = "Cancel") : Observable<boolean> {

    const config = {
      class: 'modal-sm',
      initialState: {
        title,
        message,
        btnOkText,
        btnCancelText        
      }
    }

    this.bsModalRef = this.modalService.show(ConfirmDialogComponent,config);

    return new Observable<boolean>(this.getResult());
  }

  private getResult(){
    return (observer) => {
      const subcription = this.bsModalRef.onHidden.subscribe(() => {
        observer.next(this.bsModalRef.content.result)
        observer.complete();
      });

      return {
        unsubscribe(){
          subcription.unsubscribe();
        }
      }
    }
    
  }

}
