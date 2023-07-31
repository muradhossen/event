import { User } from "./user";

export class UserParams {
    pageNumber: number = 1;
    pageSize: number = 3;
    gender: string;
    minAge: number = 18;
    maxAge: number = 99;

    constructor(user: User) {
        this.gender = user.gender == 'male' ? 'female' : 'male';
    }
}
