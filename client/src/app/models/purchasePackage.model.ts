import { BusketModel } from "./busket.model"
import { PackageModel } from "./package.model"


export class PurchasePackagesModel {
    purchaseId?: number
    purchase?:BusketModel
    packageId?: number
    package?:PackageModel
    quantity?: number
}