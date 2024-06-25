import * as ROUTES from '../constants/Routes';

const RefreshLogin = async (navigate) => {

    const RefreshLoginToken = async () => {            
        const userId = JSON.parse(localStorage.getItem('userId'))?.userId || null;
        const url = JSON.parse(localStorage.getItem('url')) || null;
    
        const objMap = { userId };
        const finalBody = JSON.stringify(objMap);
    
        const redirect = () => {
            localStorage.clear();
            localStorage.setItem('RefreshFailed', JSON.stringify(true));
            navigate(ROUTES.LOGIN);
            return false;
        }
        
        try{
            // Fetch data
            const response = await fetch(url + '/authentication/refresh-token', {
                method: 'PUT',
                include: 'cors',
                credentials: 'include',
                body: finalBody,
                headers: {
                    'Content-Type': 'application/json',
                },
            });
            
            if(response.ok){
                return true;
            }
    
            //Clear otherwise and return to login - avoiding potential for loops
            redirect();
        }catch(error){
            redirect();
        }
    }
    return await RefreshLoginToken();
}    

export default RefreshLogin;