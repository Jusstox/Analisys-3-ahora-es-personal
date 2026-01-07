<?php
include 'db_connect.php';

$session_id = $_POST['session_id'];

$stmt = $conn->prepare("INSERT INTO Game_Runs (GameSession_ID) VALUES (?)");
$stmt->bind_param("i", $session_id);

if ($stmt->execute()) {
    echo $stmt->insert_id;
} else {
    echo "Error: " . $stmt->error;
}
$stmt->close();
$conn->close();
?>