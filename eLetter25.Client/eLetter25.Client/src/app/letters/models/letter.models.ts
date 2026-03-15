export interface AddressDto {
  street: string;
  postalCode: string;
  city: string;
  country: string;
}

export interface CorrespondentDto {
  name: string;
  address: AddressDto;
  email?: string | null;
  phone?: string | null;
}

export interface CreateLetterRequest {
  subject: string;
  sentDate: string;
  sender: CorrespondentDto;
  recipient: CorrespondentDto;
  tags: string[];
}

export interface CreateLetterResult {
  letterId: string;
  documentId: string;
}

export interface LetterSummary {
  id: string;
  subject: string;
  sentDate: string;
  createdDate: string;
  senderName: string;
  recipientName: string;
  tags: string[];
}

export interface GetLettersResult {
  letters: LetterSummary[];
}

