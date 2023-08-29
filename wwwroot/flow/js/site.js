// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function signIn(){
    fetch('https://localhost:7221/api/Account/Login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            "userName": "zhangsan",
            "password": "zhangsan1234"
        })
    }).then(res => res.json())
        .then(res => {
            console.log(res);
        })
}
