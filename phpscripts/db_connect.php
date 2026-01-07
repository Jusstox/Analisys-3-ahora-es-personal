<?php
$servername = "localhost";
$username = "marcac6";
$password = "m2ZYxQvwy2Gc";
$dbname = "marcac6";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);

// Check connection
if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}
?>