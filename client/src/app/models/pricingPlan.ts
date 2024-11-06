export interface PricingPlan {
    id: number;
    name: string;
    label: string;
    description: string;
    priceInCredits: number;
    adActiveDays: number;
}