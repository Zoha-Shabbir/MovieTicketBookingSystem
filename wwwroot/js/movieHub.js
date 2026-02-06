
        var connection = new signalR.HubConnectionBuilder()
            .withUrl("/movieHub")
            .build();

        connection.start().then(function () {
            console.log("SignalR Connected");
        }).catch(function (err) {
            console.error(err.toString());
        });

        connection.on("ReceiveTicketUpdate", function (movieId, ticketsLeft) {
            var ticketSpan = document.getElementById("tickets-" + movieId);
            if (ticketSpan) {
                ticketSpan.innerText = "Tickets Available: " + ticketsLeft;
            }

            var bookBtn = document.getElementById("book-" + movieId);
            if (bookBtn) {
                if (ticketsLeft <= 0) {
                    bookBtn.classList.add("disabled");
                    bookBtn.setAttribute("disabled", "disabled");
                } else {
                    bookBtn.classList.remove("disabled");
                    bookBtn.removeAttribute("disabled");
                }
            }
        });


        connection.on("ReceivePriceUpdate", function (movieId, newPrice) {
            var priceSpan = document.getElementById("price-" + movieId);
            if (priceSpan) priceSpan.innerText = "$" + newPrice;
        });

 
        connection.on("ReceiveMovieAdded", function (movieId, title, genre, price, posterUrl, ticketsAvailable) {
            console.log("New movie added: " + title);
        });

        connection.on("ReceiveMovieUpdated", function (movieId, price, ticketsAvailable) {
            var priceSpan = document.getElementById("price-" + movieId);
            var ticketSpan = document.getElementById("tickets-" + movieId);
            var bookBtn = document.getElementById("book-" + movieId);4

            if (priceSpan) priceSpan.innerText = "$" + price;
            if (ticketSpan) ticketSpan.innerText = "Tickets Available: " + ticketsAvailable;
            if (bookBtn) {
                if (ticketsAvailable <= 0) {
                    bookBtn.classList.add("disabled");
                    bookBtn.setAttribute("disabled", "disabled");
                } else {
                    bookBtn.classList.remove("disabled");
                    bookBtn.removeAttribute("disabled");
                }
            }
        });

        connection.on("ReceiveMovieDeleted", function (movieId) {
            var movieCard = document.getElementById("movie-" + movieId);
            if (movieCard) movieCard.remove();
        });

