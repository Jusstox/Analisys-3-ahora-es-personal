<?php
include 'db_connect.php';

$run_id = $_POST['run_id'];
$enemy_id = isset($_POST['enemy_id']) && $_POST['enemy_id'] != "" ? $_POST['enemy_id'] : NULL;
$x = $_POST['x']; $y = $_POST['y']; $z = $_POST['z'];

$stmt = $conn->prepare("INSERT INTO Death_Event (GameRun_ID, Enemy_ID, Pos_x, Pos_y, Pos_z) VALUES (?, ?, ?, ?, ?)");
$stmt->bind_param("iiddd", $run_id, $enemy_id, $x, $y, $z);

if ($stmt->execute()) echo "Death recorded";
else echo "Error: " . $stmt->error;

$conn->close();
?>