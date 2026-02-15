// wwwroot/js/notifications.js

function showNotification(message) {
    // Create a notification container if it doesn't exist
    let notificationContainer = document.getElementById('notification-container');
    if (!notificationContainer) {
        notificationContainer = document.createElement('div');
        notificationContainer.id = 'notification-container';
        notificationContainer.style.position = 'fixed';
        notificationContainer.style.top = '20px';
        notificationContainer.style.right = '20px';
        notificationContainer.style.backgroundColor = '#4CAF50'; // Green
        notificationContainer.style.color = 'white';
        notificationContainer.style.padding = '15px';
        notificationContainer.style.borderRadius = '5px';
        notificationContainer.style.boxShadow = '0 0 10px rgba(0,0,0,0.1)';
        notificationContainer.style.zIndex = '1000';
        document.body.appendChild(notificationContainer);
    }

    // Set the message and display
    notificationContainer.innerText = message;
    notificationContainer.style.display = 'block';

    // Hide the notification after a few seconds
    setTimeout(() => {
        notificationContainer.style.display = 'none';
    }, 3000); // Adjust the delay as needed
}

