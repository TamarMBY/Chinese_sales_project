export class PackageModel{
    id?:number;
    name?: string;
    description?:string;
    quantity?:number;
    price?:number;


// UI/Display fields for PrimeNG card view
    color?: string; // background color for the card
    popular?: boolean; // show 'פופולרי' ribbon
    best?: boolean; // show 'הכי משתלם' ribbon
    topLabel?: string; // top label text
    benefits?: string[]; // list of benefits
    gift?: string[]; // list of gifts
}