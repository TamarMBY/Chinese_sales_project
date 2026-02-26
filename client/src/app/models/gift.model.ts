import { DonorModel } from "./donor.model";
import { UserModel } from "./user.model";

export class GiftModel {
    id?: number;
    name?: string;
    description?: string;
    price?: number;
    imageUrl?: string;
    tickets?: []

    categoryId?: number;
    donorId?: string;
    donor?: DonorModel;
    winnerId?: string;
    winner?: UserModel;
    isDrawn?: boolean;
}