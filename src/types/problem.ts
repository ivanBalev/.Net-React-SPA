type Error = {
    [name: string]: string[];
}

// Standard accepted format of error
// in the community
type Problem = {
    type: string;
    title: string;
    status: number;
    errors: Error;
}

export default Problem;