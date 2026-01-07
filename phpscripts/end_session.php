<?php
include 'db_connect.php';

$session_id = $_POST['session_id'];

$stmt = $conn->prepare("UPDATE Game_Sessions SET end_time = NOW() WHERE session_id = ?");
$stmt->bind_param("i", $session_id);

if ($stmt->execute()) {
    echo "Session ended successfully";
} else {
    echo "Error: " . $stmt->error;
}

$stmt->close();
$conn->close();
?>