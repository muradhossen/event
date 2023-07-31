import { User } from "./user";

export class UserParams {
    pageNumber: number = 1;
    pageSize: number = 6;
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;
    orderBy : string = 'lastActive';

    constructor(user: User) {
        this.gender = user.gender == 'male' ? 'female' : 'male';
    }
}
