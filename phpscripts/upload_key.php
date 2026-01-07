<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO Key_Event (GameRun_ID, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?)");
$stmt->bind_param("iddd", $run_id, $x, $y, $z);

if ($stmt->execute()) echo "Key recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>