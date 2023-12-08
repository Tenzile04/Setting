﻿let deleteBtns = document.querySelectorAll(".delete-btn");

deleteBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();



    let url = btn.getAttribute("href");



    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            fetch(url)
                .then(response => {
                    if (response.status == 200) {
                        window.location.reload(true);
                    } else {
                        alert("tapilmadi")
                    }
                })

        }
    });
}))

//Add to basket
//let addToBasketBns = document.querySelectorAll(".add-to-basket");

//addToBasketBns.forEach(btn => btn.addEventListener("click", function (e) {
//        let url = btn.getAttribute("href");

//    e.preventDefault();


//    Swal.fire({
//        position: "top-end",
//        icon: "success",
//        title: "Added To Basket",
//        showConfirmButton: false,
//        timer: 1500
//    }).then((result) => {
//        if (result.isConfirmed) {
//            fetch(url)
//                .then(response => {
//                    if (response.status == 200) {
//                        window.location.reload(true);
//                    } else {
//                        alert("error")
//                    }
//                })
//        }
//    });

//}))
let addToBasketBns = document.querySelectorAll(".add-to-basket");


addToBasketBns.forEach(btn => btn.addEventListener("click", function (e) {
    let url = btn.getAttribute("href");

    e.preventDefault();

    fetch(url)
        .then(response => {
            if (response.status == 200) {
                alert("Added to basket")
            } else {
                alert("error!")
            }
        })


}))
