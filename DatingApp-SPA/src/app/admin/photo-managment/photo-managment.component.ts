import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyjsService } from 'src/app/_services/alertifyjs.service';

@Component({
  selector: 'app-photo-managment',
  templateUrl: './photo-managment.component.html',
  styleUrls: ['./photo-managment.component.css'],
})
export class PhotoManagmentComponent implements OnInit {
  photos: any;
  constructor(
    private adminService: AdminService,
    private alertify: AlertifyjsService
  ) {}

  ngOnInit() {
    this.getPhotoForApproval();
  }

  getPhotoForApproval() {
    this.adminService.getPhotosForApproval().subscribe(
      (photos) => {
        this.photos = photos;
      },
      (error) => {
        console.log(error);
      }
    );
  }

  approvePhoto(photoId: string) {
    this.adminService.approvePhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        );
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }

  rejectPhoto(photoId: string) {
    this.adminService.rejectPhoto(photoId).subscribe(
      () => {
        this.photos.splice(
          this.photos.findIndex((p) => p.id === photoId),
          1
        );
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
