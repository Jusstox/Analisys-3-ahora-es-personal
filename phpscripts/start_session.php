<?php
include 'db_connect.php';

$player_id = isset($_POST['player_id']) ? $_POST['player_id'] : 1;

$stmt = $conn->prepare("INSERT INTO Game_Sessions (player_id) VALUES (?)");
$stmt->bind_param("i", $player_id);

if ($stmt->execute()) {
    echo $stmt->insert_id;
} else {
    echo "Error: " . $stmt->error;
}
$stmt->close();
$conn->close();
?>