function roadRating(){
const maxValue = 5;
const minValue = 0;
const unitWidth = 70 / (maxValue - minValue); // 根據最大值調整寬度

// 獲取所有的 star-rating-div 元素
const rectangles = document.querySelectorAll(".rectangle");

// 遍歷所有的 star-rating-div 元素並設置寬度
rectangles.forEach(rectangle => {
    const value = parseFloat(rectangle.parentElement.querySelector('#rating-value').value); // 獲取相對應的 value 屬性值
    const width = (value - minValue) * unitWidth;
    rectangle.style.width = width + "px";
});
}

