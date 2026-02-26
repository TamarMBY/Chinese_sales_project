import { PurchasePackagesModel } from "./purchasePackage.model";
import { TicketModel } from "./ticket.model";

export class BusketModel{
    id?: number;
    buyerId?: string;
    totalAmount?: number;
    orderDate?: Date;
    isDraft?: boolean;
    purchasePackages?: PurchasePackagesModel[];
    tickets?: TicketModel[];
}