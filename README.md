## Program Analysis Using Constriants - LLVM Analysis

### Overview

I was curious if the analyses described in the paper could be applied to a lower-level representation (namely LLVM), so they could be generalized accross various programming languages that target LLVM.

Since I've already taken CSC 431, I decided to start with my "Mini" compiler and build my experiments from that. To avoid risks with academic dishonesty, I've only included new files in this repository. Unfortunately, that means that this code won't run unless you have the rest of my compiler, but the bulk of my work is demonstrated with these new files.

### Findings

The extent of my progress was "finishing" constraint generation (there are still lots of improvements to be made). I would have loved to dive into constriant solving, but unfortunately my time was sucked away by working around the oddities of generating constraints for a low-level representation.

Without repeating what the paper says, I'll point out some interesting challenges I encountered with constraint generation for LLVM.

First, compilation to LLVM has the potential to mangle the control flow of the program in minor but annoying ways. For example, here is my compiler's (unoptomized) translation of the PV1 function (from the paper) to LLVM:

```llvm
define void @PV1(i32 %y) {
LU0:
	%u0 = mul i32 50, -1
	%u1 = icmp slt i32 %u0, 0
	br i1 %u1, label %LU3, label %LU2
LU3:
	%y0 = phi i32 [%y, %LU0], [%u3, %LU3]
	%x0 = phi i32 [%u0, %LU0], [%u2, %LU3]
	%u2 = add i32 %x0, %y0
	%u3 = add i32 %y0, 1
	%u4 = icmp slt i32 %u2, 0
	br i1 %u4, label %LU3, label %LU2
LU2:
	%y1 = phi i32 [%y, %LU0], [%u3, %LU3]
	%u5 = icmp sgt i32 %y1, 0
	; assert %u5
	br label %LU1
LU1:
	ret void
}
```

`LU3` is the body of the while loop. Notice how there is a conditional branch both at the end of `LU0` and `LU3`. This is equivalent to writing the `while` loop like this:

```c
if (x < 0) {
    do {
        // ...
    } while (x < 0);
}
```

This change immediately increases the complexity of the generated paths, since the first `if` statement adds a direct path from the entry cut point to the exit cut point. Furthermore, since the loop's conditional branch occurs at the end of its body rather than the top (where the cut point is), this creates a path from the end of the loop to the exit cut point in addition to the cut point at the top of the loop. Of course, there are probably some additional analyses or more optimal cut-point placements I didn't explore that could mitigate this problem.

Another new level of complexity arises from the fact that the generated LLVM uses intermediate values liberally, especially when performing conditional branches. Since a virtual register is allocated for the comparison before every conditional branch, that's another variable we need to consider when generating constraints.

The last interesting challenge was mapping virtual registers to variables. The generated LLVM shown above is in single static assignment (SSA) form, which means every instruction result gets assigned to a new, unused virtual register. This poses a challenge when different paths through the control flow graph use different registers to refer to the same value. Thankfully, the LLVM `phi` instruction helps to solve this problem by indicating which virtual registers refer to the same value when coming from different "basic blocks".

Overall, performing program analysis using constraints does in fact seem possible for low-level code, but the constraints generated tend to be more complex when compared to constraints generated directly from the AST representation. Future work could involve implementing constraint solving and seeing how analyzing LLVM performs against analyzing an AST.
