// Escape regular expression
function escapeRegExp(string) {
    return string.replace(/[.*+\-?^${}()|[\]\\]/g, '\\$&');
}

//function chatAI(string) {
//    //alert($("#chat").val());
//    return "cxvcxv";
//}

// Initiate GET request to url provided
$('[data-get]').click(e => {
    e.preventDefault();
    let url = $(e.target).data('get');
    location = url || location;
});


$('#startAI').click(e => {
    $('#startAI').hide();

    $("#all").removeAttr("hidden");
    //$("#aichatE2").removeAttr("hidden");
    //$("#aichatE3").removeAttr("hidden");
    //alert($("#aichatEl").show());
    //$("#aichatEl").show();
    //$("#aichatE2").show();
    //$("#aichatE3").show();
    let chat_dict = {



        'Payment': {
            "problem": {
                "P1": ["<div value='P1'> Where to make a payment? </div> "],
                "P2": ["B:What payment method can use in making payment ?"]



            },
            "solution": {
                "P1": ["<div value='P1'> Where to make a payment? </div> "],
                "P2": ["B:What payment method can use in making payment ?"]
            }
        },
        'Booking': {
            "problem": {
                'P1': ["A:Where to book time for visit property?"],
                'P2': ["B:Where  can view the booked property's status ?"],
                'P3': ["C:What date  and time is  available to be book?"]
            },
            "solution": {
                "P1": ["A:How to view all user  payment history ?"],
                "P2": ["B:What type of data able to be view in  payment history ?"],
                'P3': ["C:What date  and time is  available to be book?"]
            }

        }

    };

    let chat_dictAdmin = {



        'Payment': {
            "problem": {
                "P1": ["<div value='P1'> Where to make a payment? </div>"],
                "P2": ["B:What type of data able to be view in  payment history ?"]



            },
            "solution": {
                "P1": ["A:How to view all user  payment history ?"],
                "P2": ["B:What type of data able to be view in  payment history ?"]
            }
        },
        'Booking': {
            "problem": {
                'B1': ["C:Where to view  all  user booking records?"]
            },
            "solution": {
                "B1": ["A:How to view all user  payment history ?"]
            }

        }

    };
    let chat_dictOwner = {



        'Payment': {
            "problem": {
                "P1": ["<div value='P1'> Where to make a payment? </div>"],
                "P2": ["Problem2:What should be include in formal receipt ?"]



            },
            "solution": {
                //< li > <a href="~/Account/AIChat" id="chatAi"> AI Chat </a></li>
                "P1": ["Problem1:How to view  payment details make by students?"],
                "P2": ["Problem2:What should be include in formal receipt ?"]
            }
        },
        'Booking': {
            "problem": {
                'B1': ["Problem1:How to  view all students booking records?"]
            },
            "solution": {
                //< li > <a href="~/Account/AIChat" id="chatAi"> AI Chat </a></li>
                "B1": ["Problem1:How to view  payment details make by students?"],

            }

        }

    };

    
    //var values = ["dog", "cat", "parrot", "rabbit"];
    var values = [];

    $('#drop')
        .append(
            $(document.createElement('label')).prop({
                for: 'cat'
            }).html('Choose your question categories : ')
        )
        .append(
            $(document.createElement('select')).prop({
                id: 'cat',
                name: 'cat'
            })
        )

    for (var i in chat_dict) {

        values.push(i);
      
 
         //alert("value" + values[0]);
        $('#cat').append($(document.createElement('option')).prop({
            value: i,
            text: i
        }))
      
     
    }

    //start button chat then hide start button then display the input and button 

   // $('#chat1').prepend(`<div style=" width:100%" ><b>Categories</b></br><b> 1.Payment</b></br><b>2.Booking</b></br><b>P : </b>

   //</div>`);
});
$('#aichat').click(e => {
    e.preventDefault();
  
    //alert("3"+e.target.value);
    let cls = "asker";


    $('#chat1').prepend(`<div class="${cls}" ><b>${$("#chat").val().toUpperCase().trim()}:</b>

   </div>`);
    //sender question
  
    let chat_dict = {
    


        'Payment': {
            "problem": {
                "P1": [" make payment "," pay the booking "," checkout "," to pay ", " make a payment "," pay a bill "],
                "P2": [" payment method "," payment option "," payment options "]



            },
            "solution": {
                "P1": ["If does not have </br>any booking </br>pls add new booking </br>before make payment</br> else </br> direct go to  <a href='Order'>Order</a> page " ],
                "P2": ["Payment method can be use is Card and Fund transfer only !"]
            }
        },
        'Booking': {
            "problem": {
                'P1': ["<div>Where to book time for visit property?</div>"],
                'P2': ["<div>Where  can view the booked property's status? </div>"],
                'P3': ["<div>What date  and time is  available to be book?</div>"]
            },
            "solution": {
                "P1": ["Go <a href='Property'>Property</a> </br> Page then select a </br>property to book"],
                "P2": ["Direct go to  <a href='Order'>Order</a> page</br> to view status(pending/paid)"],
                'P3': ["Only date and time </br>after today can be book </br>and cannot book</br> if the time slot</br> that day been </br>booked by other students"]
            }

        }

    };

    let chat_dictAdmin = {
  


        'Payment': {
            "problem": {
                "P1": ["<div value='P1'> Where to make a payment? </div>"],
                "P2": ["B:What type of data able to be view in  payment history ?"]



            },
            "solution": {
                "P1": [" "],
                      //< li > <a href="~/Account/AIChat" id="chatAi"> AI Chat </a></li>
                "P2": ["B:What type of data able to be view in  payment history ?"]
            }
        },
        'Booking': {
            "problem": {
                'B1': ["C:Where to view  all  user booking records?"]
            },
            "solution": {
                "B1": ["A:How to view all user  payment history ?"]
            }

        }

    };
    let chat_dictOwner = {



        'Payment': {
            "problem": {
                "P1": ["<div> Where to make a payment? </div>"],
                "P2": ["Problem2:What should be include in formal receipt ?"]



            },
            "solution": {
                      //< li > <a href="~/Account/AIChat" id="chatAi"> AI Chat </a></li>
                "P1": ["Problem1:How to view  payment details make by students?"],
                "P2": ["Problem2:What should be include in formal receipt ?"]
            }
        },
        'Booking': {
            "problem": {
                'B1': ["Problem1:How to  view all students booking records?"]
            },
             "solution": {
                //< li > <a href="~/Account/AIChat" id="chatAi"> AI Chat </a></li>
                "B1": ["Problem1:How to view  payment details make by students?"],
         
            }

        }

    };

  // this is direct search solution 
    let input = $("#chat").val().toUpperCase();

    let solut = false;
    let solutOwner = false;
    let solutAdmin = false;

    if ($('#cat').val() != "" && e.target.value == "Student") {
     

        for (var k in chat_dict[$('#cat').val()]["problem"]) {


            for (var f in chat_dict[$('#cat').val()]["problem"][k]) {
          
                let solution = chat_dict[$('#cat').val()]["solution"][k];
                let str1 = chat_dict[$('#cat').val()]["problem"][k][f];


               str1= str1.toUpperCase();
                input = input + " ";
  
            
                if (input.includes(str1) == true) {
                   
                    solut = true;
                  
                    $('#chat1').append(`<div style="margin-left:80%; width:100%;height:100%" ><b>${solution}</b>

   </div>`);
                }
               
            }
         
          
        }

        if (solut == false) {
            alert("Please input other keyword e.g. payment !");
        }
    }


    if ($('#cat').val() != "" && e.target.value == "Owner") {


        for (var k in chat_dictOwner[$('#cat').val()]["problem"]) {



            for (var f in chat_dictOwner[$('#cat').val()]["problem"][k]) {
           
                let solution = JSON.stringify(chat_dictOwner[$('#cat').val()]["solution"][k]);
                let str1 = chat_dictOwner[$('#cat').val()]["problem"][k][f];


                str1 = str1.toUpperCase();
                input = input + " ";
            
                if (input.includes(str1) == true) {
                
                    solut = true;
                    $('#chat1').append(`<div style="margin-left:80%; width:100%;height:100%" ><b>${solution}</b>

   </div>`);
                }
             
            }


        }
        if (solutOwner == false) {
            alert("Please input other keyword e.g. payment !");
        }


    }

    if ($('#cat').val() != "" && e.target.value == "Admin") {



        for (var k in chat_dictAdmin[$('#cat').val()]["problem"]) {



            for (var f in chat_dictAdmin[$('#cat').val()]["problem"][k]) {

                let solution = JSON.stringify(chat_dictAdmin[$('#cat').val()]["solution"][k]);
                let str1 = chat_dictAdmin[$('#cat').val()]["problem"][k][f];


                str1 = str1.toUpperCase();
                input = input + " ";

                if (input.includes(str1) == true) {

                    solut = true;
                    $('#chat1').append(`<div style="margin-left:80%; width:100%;height:100%" ><b>${solution}</b>

   </div>`);
                }
              
            }


        }
        if (solutAdmin == false) {
            alert("Please input other keyword e.g. payment !");
        }


    }
    

        
       
    



        //if (input == i.toUpperCase().trim()) {

        //}
  

    
    ////if cat match e.g. payment then display payment question -> user will choose P1 e.g. then match with P1
   // for (var i in chat_dict) {
   //     $('#chat1').append(`<div style="margin-left:60%; width:60%" ><a href="#">${i}:</b>

   //</div>`);
   // }
    //    //print myDict['key1']['attr3']
    //    for (var k in chat_dict[i]) {
    //        //i is payment/booking ..... k is problem/solution
    //        //cat = payment  ->chat_dict[cat][problem][]
    //        alert("sss" + chat_dict["Payment"]["problem"]["P1"])
    //        alert("k" + k);
    //        alert("aaa" + chat_dict[k]);
         
    //        if (input == i.toUpperCase().trim()) {
    //            alert("dd" + i + "sss" + k);
               

    //            alert("yes");
    //        } else {
    //            alert("No");
    //        }
    //    }
    //}
    //let cat = "none";
     //if (e.target.value == "Student") {
   
   
 



});

// Auto-upper
$('[data-upper]').on('input', e => {
    let a = e.target.selectionStart;
    let b = e.target.selectionEnd;
    e.target.value = e.target.value.toUpperCase();
    e.target.setSelectionRange(a, b);
});

// Reset form
$('[type=reset]').click(e => {
    e.preventDefault();
    location = location;
});

// Initiate POST request to url provided
$('[data-post]').click(e => {
    e.preventDefault();
    let url = $(e.target).data('post');

    let f = $('<form>')[0];
    f.method = 'post';
    f.action = url || location;
    $(document.body).append(f);
    f.submit();
});

// Check all checkboxes
$('[data-check]').click(e => {
    e.preventDefault();
    let name = $(e.target).data('check');
    $(`[name=${name}]`).prop('checked', true);
});

// Uncheck all checkboxes
$('[data-uncheck]').click(e => {
    e.preventDefault();
    let name = $(e.target).data('uncheck');
    $(`[name=${name}]`).prop('checked', false);
});

// Make table row checkable (first checkbox)
$('[data-checkable]').click(e => {
    if ($(e.target).is('input,button,a')) return;

    let cb = $(e.currentTarget).find(':checkbox')[0];
    if (cb) {
        cb.checked = !cb.checked;
    }
});