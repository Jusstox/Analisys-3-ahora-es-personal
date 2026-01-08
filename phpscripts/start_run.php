<?php
include 'db_connect.php';

ini_set('display_errors', 1);
error_reporting(E_ALL);

$session_id = $_POST['session_id'];
$is_restart = isset($_POST['isRestart']) ? $_POST['isRestart'] : 0; 

$sql = "INSERT INTO Game_Runs (GameSession_ID, IsRestart) VALUES (?, ?)";

$stmt = $conn->prepare($sql);

if (!$stmt) {
    die("SQL Prepare Error: " . $conn->error);
}

$stmt->bind_param("ii", $session_id, $is_restart);

if ($stmt->execute()) {
    echo $stmt->insert_id;
} else {
    echo "Insert Error: " . $stmt->error;
}

$stmt->close();
$conn->close();
?>