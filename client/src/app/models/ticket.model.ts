import { BusketModel } from "./busket.model";
import { GiftModel } from "./gift.model";

export class TicketModel {
    id?: number;
    giftId?: number;
    busketId?: number;
    gift?: GiftModel;
    purchase?: BusketModel;
    quantity?: number = 1;
    totalTickets?: number;
}
