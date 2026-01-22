export class GiftModel {
    id?: number;
    name?: string;
    description?: string;
    price?: number;
    imageUrl?: string;

    categoryId?: number;
    donorId?: string;
    winnerId?: string
    isDrawn?: boolean;
}