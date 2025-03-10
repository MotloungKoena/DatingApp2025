import { Component } from '@angular/core';
import { Developer } from '../_models/developer';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.css']
})
export class ContactUsComponent {
  developer: Developer = {
    developerName: 'Koena Motloung',
    developerEmail: '2019476593@ufs4life.ac.za',
    developerBio: 'A passionate developer specializing in C# and Angular.'
  };
}
