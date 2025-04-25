interface Prop{
    message:string;
}


function Loading({message}:Prop){
    return <h1>{message}</h1>
}

export default Loading;