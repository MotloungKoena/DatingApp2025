import { Photo } from './photo'; // Import the existing model

export interface Visit {
  id: number;
  username: string;
  knownAs: string;
  age: number;
  gender: string;
  photoUrl: string;
  city: string;
  country: string;
  created: Date;
  lastActive: Date;
  visitedOn: Date;
  introduction: string;
  interests: string;
  lookingFor: string;
  photos: Photo[]; 
}
