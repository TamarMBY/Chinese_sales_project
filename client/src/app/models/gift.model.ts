import { DonorModel } from "./donor.model";

export class GiftModel {
    id?: number;
    name?: string;
    description?: string;
    price?: number;
    imageUrl?: string;

    categoryId?: number;
    donorId?: string;
    donor?: DonorModel;
    winnerId?: string
    isDrawn?: boolean;
}