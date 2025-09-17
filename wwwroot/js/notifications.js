export function requestNotificationPermission() {
    if (typeof Notification !== "undefined" && Notification.permission !== "granted") {
        Notification.requestPermission();
    }
}

export function showNotification(title, body) {
    if (Notification.permission === 'granted') {

        new Notification(title, { body });

    } else if (Notification.permission !== 'denied') {

        Notification.requestPermission()
            .then(permission => {
                if (permission === 'granted') {
                    new Notification(title, { body })
                }
            }); 

    }
}

export function playNotificationSound() {
    const audio = new Audio('sounds/confirmation-notification.wav');
    audio.play();
}
