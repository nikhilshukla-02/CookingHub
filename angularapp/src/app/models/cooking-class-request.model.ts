export interface CookingClassRequest {
    CookingClassRequestId?: number;
    UserId: number;
    CookingClassId: number;
    RequestDate: string;      // ISO date string
    Status: string;            // "Pending", "Approved", "Rejected"
    DietaryPreferences: string;
    CookingGoals: string;
    Comments?: string;
}