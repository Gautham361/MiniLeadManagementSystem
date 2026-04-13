export type LeadStatus = "New" | "InProgress" | "Converted" | "Lost";

export interface Lead {
  leadId: number;
  leadName: string;
  companyName: string;
  email: string;
  phoneNumber: string;
  status: LeadStatus;
  assignedTo: string;
}