function jump(element) {
    element.classList.add('jump-animation');
    setTimeout(() => {
        element.classList.remove('jump-animation');
    }, 300);
}