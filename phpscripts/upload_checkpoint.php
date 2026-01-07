<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$name = $_POST['name'];
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO Checkpoint_Event (GameRun_ID, Checkpoint_name, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?, ?)");
$stmt->bind_param("isddd", $run_id, $name, $x, $y, $z);

if ($stmt->execute()) echo "Checkpoint recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>