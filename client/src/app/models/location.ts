export interface City {
    id: number;
    name: string;
    postalCode: string;
    cantonId?: number;
    countryId: number;
}

export interface Country {
    id: number;
    name: string;
}

// export interface Municipality {
//     id: number;
//     name: string;
//     cityId: number;
// }

// export interface Canton {
//     id: number;
//     name: string;
//     countryId: number;
// }
