import { Component, inject } from '@angular/core';
import { GiftComponent } from '../gift.component';
import { GiftService } from '../../../services/gift.service';
import { GiftModel } from '../../../models/gift.model';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-get-gift',
  imports: [],
  templateUrl: './get-gift.component.html',
  styleUrl: './get-gift.component.css'
})
export class GetGiftComponent {
  giftSrv: GiftService = inject(GiftService);
  private route = inject(ActivatedRoute);
  giftId!: number;
  ngOnInit() {
    this.giftId = Number(this.route.snapshot.paramMap.get('id'));
  }
  draftGift: GiftModel = {};
  getById(){
    this.giftSrv.getById(this.giftId).subscribe(gift => {
      this.draftGift = gift;
    });
  }
}
