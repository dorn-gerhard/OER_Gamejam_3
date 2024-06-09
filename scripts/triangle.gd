class_name Triangle
extends Node

var leg_a: TriangleLine2D
var leg_b: TriangleLine2D
var hypothenuse: TriangleLine2D

var alpha: TriangleAngle2D
var beta: TriangleAngle2D


static func has_right_angle(point_a: Point, point_b: Point, point_c: Point) -> bool:
	return 	point_a.has_angle_approx(PI/2) or \
			point_b.has_angle_approx(PI/2) or \
			point_c.has_angle_approx(PI/2)


static func from_points(points: Array[Point]) -> Triangle:
	var tri := Triangle.new()
	var longest_pair = [points[0], points[1]]
	if (points[1].position - points[2].position).length() > \
		(longest_pair[0].position - longest_pair[1].position).length():
			longest_pair = [points[1], points[2]]
	if (points[2].position - points[0].position).length() > \
		(longest_pair[0].position - longest_pair[1].position).length():
			longest_pair = [points[2], points[0]]
	
	var point_c: Point
	for point in points:
		if not point in longest_pair:
			point_c = point
	
	tri.hypothenuse = LevelManager.lines[longest_pair]
	tri.leg_a = LevelManager.lines[[longest_pair[0], point_c]]
	tri.leg_b = LevelManager.lines[[longest_pair[1], point_c]]
	
	print([longest_pair[0], longest_pair[1], point_c])
	print(LevelManager.angles)
	
	if [longest_pair[0], longest_pair[1], point_c] in longest_pair[1].angles:
		tri.alpha = longest_pair[1].angles[[longest_pair[0], longest_pair[1], point_c]]
	else:
		tri.alpha = longest_pair[1].angles[[point_c, longest_pair[1], longest_pair[0]]]
	
	if [point_c, longest_pair[0], longest_pair[1]] in longest_pair[0].angles:
		tri.beta = longest_pair[0].angles[[point_c, longest_pair[0], longest_pair[1]]]
	else:
		tri.beta = longest_pair[0].angles[[longest_pair[1], longest_pair[0], point_c]]
	
	return tri


static func from_angle_and_line(angle: TriangleAngle2D, line: TriangleLine2D) -> Triangle:
	var tri := Triangle.from_points(angle.get_points())
	if line in [tri.leg_a, tri.leg_b, tri.hypothenuse]:
		return tri
	else:
		return null


static func from_two_lines(lines: Array[TriangleLine2D]) -> Triangle:
	var tri := Triangle.new()
	var shared_points := lines[0].get_shared_points_with(lines[1])
	if shared_points.is_empty():
		return null
	
	return Triangle.from_points(shared_points)
