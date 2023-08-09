//// 假設你的數值儲存在名為 'value' 的變數中

//const value = document.getElementById('rating-value').value; // 請將這裡的數值替換為你的實際數值
//const rectangle = document.querySelector(".star-rating-div");
//const maxValue = 5;
//const minValue = 0;
//const unitWidth = 50/ (maxValue - minValue); // 根據最大值調整寬度
//// 根據數值計算矩形的寬度
//const width = (value - minValue) * unitWidth;
//rectangle.style.width = width + "px";
const maxValue = 5;
const minValue = 0;
const unitWidth = 50 / (maxValue - minValue); // 根據最大值調整寬度

// 獲取所有的 star-rating-div 元素
const rectangles = document.querySelectorAll(".star-rating-div");

// 遍歷所有的 star-rating-div 元素並設置寬度
rectangles.forEach(rectangle => {
    const value = parseFloat(rectangle.parentElement.querySelector('#rating-value').value); // 獲取相對應的 value 屬性值
    const width = (value - minValue) * unitWidth;
    rectangle.style.width = width + "px";
});
