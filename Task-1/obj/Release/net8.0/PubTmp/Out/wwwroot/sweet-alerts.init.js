
window.sweetAlertInterop = {
    showSuccess: function (title, text) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'success',
            showCloseButton: true, // Show the close button
            allowOutsideClick: false, // Prevent closing by clicking outside the alert
            allowEscapeKey: false // Prevent closing by pressing the escape key
        }).then(function (result) {
            if (result.isConfirmed) {
                // Handle the close button click if needed
            }
        });
    },
    showError: function (title, text) {
        Swal.fire({         
            title: title,
            text: text,
            icon: 'error',          
            imageAlt: 'Custom image', // Specify the alt text for the custom image
            showCloseButton: true, // Show the close button
            allowOutsideClick: false, // Prevent closing by clicking outside the alert
            allowEscapeKey: false // Prevent closing by pressing the escape key
        }).then(function (result) {
            if (result.isConfirmed) {
                // Handle the close button click if needed
            }
        });
    },
    Unauthorized: function (title, text) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            showConfirmButton: true,
            allowOutsideClick: false,
            allowEscapeKey: false
        }).then(function () {
            window.location.href = "/";
        });
    },
    IncorrectLogin: function (title, text) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'warning',
            /* imageUrl: 'https://unsplash.it/400/200', // Replace with the URL of your custom image*/
            imageUrl: 'assets/images/SKLogo.png',
            imageWidth: 240, // Specify the width of the custom image
            imageHeight: 72, // Specify the height of the custom image
            imageAlt: 'Custom image', // Specify the alt text for the custom image
            showCloseButton: true,
            allowOutsideClick: false,
            allowEscapeKey: false,
            customClass: {
                closeButton: 'custom-close-button', // Add a custom class for the close button
                title: 'custom-title', // Add a custom class for the title
                content: 'custom-content' // Add a custom class for the content
            }
        }).then(function (result) {
            if (result.isConfirmed) {
                // Handle the close button click if needed
            }
        });
    },
    ShowLoading: function (title, text, duration) {
        Swal.fire({
            title: title,
            text: text,
            icon: 'html',
            iconHtml: '<i class="bx bx-buoy bx-spin text-primary display-3"></i>', // Custom HTML for the icon
            imageAlt: 'Custom image', // Specify the alt text for the custom image
            showCloseButton: false, // Show the close button
            showCancelButton: false, // Hide the cancel button
            showConfirmButton: false,
            allowOutsideClick: false, // Prevent closing by clicking outside the alert
            allowEscapeKey: false // Prevent closing by pressing the escape key
        });
         setTimeout(function() {
                // Your cancel function logic here
                // This code will run after 'duration' milliseconds
                Swal.close();
            }, duration);
    },

    //showError500: function (title, text) {
    //    Swal.fire({
    //        title: title,
    //        text: text,
    //        icon: 'error',
    //        imageAlt: 'Custom image', // Specify the alt text for the custom image
    //        showCloseButton: true, // Show the close button
    //        allowOutsideClick: false, // Prevent closing by clicking outside the alert
    //        allowEscapeKey: false // Prevent closing by pressing the escape key
    //    }).then(function (result) {
    //        if (result.isConfirmed) {
    //            // Handle the close button click if needed
    //        }
    //    });
    //},

    showError500: function (title, text) {
        const htmlCode = `
        <div class="account-pages my-3 pt-5">
            <div class="container">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="text-center mb-3">
                            <h1 class="display-2 fw-medium">5<i class="bx bx-buoy bx-spin text-primary display-3"></i>0</h1>                         
                            <p>${text}</p>                        
                        </div>
                    </div>
                </div>
            
            </div>
        </div>
    `;

        Swal.fire({
            title: title,
            html: htmlCode,
         /*   icon: 'error', // Use 'error' icon for a 5005 error message*/
            imageUrl: 'assets/images/SKLogo.png',
            imageWidth: 240,
            imageHeight: 52,
            imageAlt: 'Custom image',
         /*   showCloseButton: true,*/
            allowOutsideClick: false,
            allowEscapeKey: false,
            customClass: {
                closeButton: 'custom-close-button',
                title: 'custom-title',
                content: 'custom-content'
            }
        }).then(function (result) {
            if (result.isConfirmed) {
                // Handle the close button click if needed
            }
        });
    },
};

window.triggerButtonClick = function (buttonId) {
    var button = document.getElementById(buttonId);
    if (button) {
        button.click();
    }
};

window.downloadFile = function (filePath) {
    var anchor = document.createElement('a');
    anchor.href = filePath;
    anchor.download = filePath.split('/').pop();
    anchor.click();
};