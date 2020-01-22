using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class EmailForm : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var html = "<table style='margin-top: 50px;'>" +
        "<tr style='border: solid; border-width: thin; background-color: black;'>" +
            "<th style='color: white;'>First Name" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white;'>Last Name " +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white;'>Order Number" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white;'>Phone Order" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white;'>Order Amount" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
        "</tr>" +
        "<tr>" +
            "<td>Stephen" +
            "</td>" +
            "<td></td>" +
            "<td>Moody" +
            "</td>" +
            "<td></td>" +
            "<td>4958782" +
            "</td>" +
            "<td></td>" +
            "<td>IPhone 5" +
            "</td>" +
            "<td></td>" +
            "<td>$45" +
            "</td>" +
            "<td></td>" +
        "</tr>" +
        "<tr style='border: solid; border-width: thin; background-color: black;'>" +
            "<th style='color: white;'>Payment Type" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white'>Payment Number" +
            "</th>" +
            "<td style='width: 30px;'></td>" +
            "<th style='color: white;'>Email" +
            "</th>" +
            "<td></td>" +
        "</tr>" +
        "<tr>" +
            "<td>" +
                "Cashapp" +
            "</td>" +
            "<td style='width: 30px;'></td>" +
            "<td>" +
                "$smb99" +
            "</td>" +
            "<td style='width: 30px;'></td>" +
            "<td>" +
                "stephenmoody87@gmail.com" +
            "</td>" +
            "<td style='width: 30px;'></td>" +
        "</tr>" +
    "</table>" +
    "<table style='margin-top: 30px;'>" +
        "<tr>" +
            "<td>" +
                "<h3>Thank you for shopping with us!" +
                "</h3>" +
                "<p style='border-bottom: thin solid #000;'></p>" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td>" +
                "<p>" +
                    "We're excited that you've decided to sell your gadgets to Cellable. To print your free shipping label and packing slip, print out the attachment on this email." +
                "</p>" +
                "<p>" +
                    "Please be sure to include your packing slip inside your box so we can easily identify your items." +
                "</p>" +
            "</td>" +
        "</tr>" +
        "<tr style='margin-top: 30px;'>" +
            "<td>" +
                "<h3>Shipping to Cellable</h3>" +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td>1. Do a factory reset on the phones. This prevents missuse of data and information." +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td>2. Find a strong box with plenty of packing materials to pack your items. Please do not add any items that are not on your packing slip." +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td>3. Pack your items and packing slip in the box with plenty of packing materials. Tape your prepaid shipping label to the box. To print your prepaid shipping label click your attachment." +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td>4. Drop your box off for shipping at the nearest USPS location." +
            "</td>" +
        "</tr>" +
    "</table>" +
    "<table>" +
        "<tr style='margin-top: 30px;'>" +
            "<td>We'll let you know as soon as we receive your items. If you have any questions along the way, you can check the status of your items on our website or visit our help center." +
            "</td>" +
        "</tr>" +
        "<tr>" +
            "<td style='height: 36px'>" +
                "<p style='margin-top: 32px;'>" +
                    "Thanks,</p>" +
                        "<p style='margin-left: 35px;'>  Cellable team" +
                "</p>" +
            "</td>" +
        "</tr>" +
        "<tr style='margin-top: 50px;'>" +
            "<td style='margin-top: 30px;'>" +
                "<p style='text-align: center; margin-top: 30px; font-size: xx-small;'>" +
                    "© Copyright 2020 - Cellable LLC, All Rights Reserved, Patents Pending." +
                "</p>" +
                "<p style='text-align: center;  font-size: xx-small;'>" +
                    "Designated trademarks and brands are the property of their respective owners." +
                "</p>" +
                "<p style='text-align: center; font-size: xx-small;'>" +
                    "Cellable is not affiliated with the manufacturers of the items available for trade-in." +
                "</p>" +
                "<p style='text-align: center; font-size: xx-small;'>" +
                    "4120 Manor House Drive, Marietta GA 30062" +
                "</p>" +
            "</td>" +
        "</tr>" +
    "</table>"; 

    }
}