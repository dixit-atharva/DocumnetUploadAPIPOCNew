import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ReCaptchaV3Service    } from 'ng-recaptcha';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-google-recaptcha',
  templateUrl: './google-recaptcha.component.html',
  styleUrl: './google-recaptcha.component.css'
})
export class GoogleRecaptchaComponent {
  // Inject the service in the constructor
  constructor(private recaptchaV3Service: ReCaptchaV3Service,private http: HttpClient) {
  }
  public apiUrl: string = environment.apiUrl;
  public send(): void {
    // if (form.invalid) {
    //   for (const control of Object.keys(form.controls)) {
    //     form.controls[control].markAsTouched();
    //   }
    //   return;
    // }

    this.recaptchaV3Service.execute('importantAction')
    .subscribe((token: string) => {
      console.log(`Token [${token}] generated`);
      this.http.post(`${this.apiUrl}/DocUpload/verify`, { token: token })
        .subscribe(response => {
          console.log('reCAPTCHA verified successfully:', response);
        }, error => {
          console.error('reCAPTCHA verification failed:', error);
        });
    });
  }
  
}
